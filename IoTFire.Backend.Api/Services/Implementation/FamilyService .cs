using IoTFire.Backend.Api.Data;
using IoTFire.Backend.Api.Models.DTOs.FamillyMember;
using IoTFire.Backend.Api.Models.Entities;
using IoTFire.Backend.Api.Models.Entities.Enums;
using IoTFire.Backend.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace IoTFire.Backend.Api.Services.Implementation
{
    public class FamilyService : IFamilyService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public FamilyService(
            AppDbContext context,
            IEmailService emailService,
            IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

     
        public async Task<(bool Success, string Message)> InviteMemberAsync(
            int occupantId, InviteFamilyMemberDto dto)
        {
            // 1. Récupérer l'occupant pour personnaliser l'email
            var occupant = await _context.Users.FindAsync(occupantId);
            if (occupant == null)
                return (false, "Occupant introuvable.");

            // 2. Vérifier que l'email n'est pas déjà utilisé par un compte actif
            var existingActiveUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);
            if (existingActiveUser != null)
                return (false, "Un compte actif existe déjà avec cet email.");

            // 3. Vérifier s'il y a déjà une invitation en attente pour cet email
            //    (User inactif avec un token non expiré lié à cet occupant)
            var existingPendingInvitation = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.Email &&
                    !u.IsActive &&
                    u.ParentUserId == occupantId &&
                    u.TokenExpiration > DateTime.UtcNow);

            if (existingPendingInvitation != null)
                return (false, "Une invitation est déjà en attente pour cet email. Attendez qu'elle expire ou révoquez-la.");

            // 4. Générer un token URL-safe unique
            var rawToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                         + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var token = rawToken.Replace("+", "-").Replace("/", "_").Replace("=", "");

            // 5. Créer l'utilisateur "fantôme" (invitation en attente)
            var pendingMember = new User
            {
                FirstName = "",               // sera rempli lors de l'acceptation
                LastName = "",               // sera rempli lors de l'acceptation
                Email = dto.Email,
                PasswordHash = "",           // sera rempli lors de l'acceptation
                PhoneNumber = "",               // sera rempli lors de l'acceptation
                Role = EnumRole.FamilyMember,
                ParentUserId = occupantId,
                ResetToken = token,        // réutilisation du champ pour le token d'invitation
                TokenExpiration = DateTime.UtcNow.AddHours(48),
                IsActive = false,            // compte inactif jusqu'à acceptation
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(pendingMember);
            await _context.SaveChangesAsync();

            // 6. Construire et envoyer l'email d'invitation
            var frontendUrl = _config["App:FrontendUrl"] ?? "http://localhost:3000";
            var invitationLink = $"{frontendUrl}/accept-invitation?token={token}";

            var emailSubject = $"Invitation à rejoindre le système de {occupant.FirstName} {occupant.LastName}";
            var emailBody = BuildInvitationEmail(occupant, invitationLink);

            await _emailService.SendEmailAsync(dto.Email, emailSubject, emailBody);

            return (true, "Invitation envoyée avec succès. Le lien expire dans 48 heures.");
        }

       
        public async Task<FamilyListResponseDto> GetFamilyMembersAsync(int occupantId)
        {
            // Membres dont le compte est actif
            var activeMembers = await _context.Users
                .Where(u => u.ParentUserId == occupantId && u.IsActive)
                .Select(u => new FamilyMemberDto
                {
                    Id = u.Id,
                    LastName = u.LastName,
                    FirstName = u.FirstName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            // Invitations en attente (compte inactif, token pas encore expiré)
            var pendingInvitations = await _context.Users
                .Where(u =>
                    u.ParentUserId == occupantId &&
                    !u.IsActive &&
                    u.TokenExpiration > DateTime.UtcNow)
                .Select(u => new PendingInvitationDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    ExpiresAt = u.TokenExpiration!.Value,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return new FamilyListResponseDto
            {
                ActiveMembers = activeMembers,
                PendingInvitations = pendingInvitations
            };
        }

        
        public async Task<(bool Success, string Message)> RevokeMemberAsync(
            int occupantId, int memberId)
        {
            var member = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == memberId && u.ParentUserId == occupantId);

            if (member == null)
                return (false, "Membre introuvable ou non autorisé.");

            if (member.IsActive)
            {
                // Membre actif : désactiver son compte
                member.IsActive = false;
                member.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return (true, $"L'accès de {member.FirstName} {member.LastName} a été révoqué.");
            }
            else
            {
                // Invitation en attente : supprimer le User "fantôme"
                _context.Users.Remove(member);
                await _context.SaveChangesAsync();
                return (true, $"L'invitation pour {member.Email} a été annulée.");
            }
        }

        
        public async Task<(bool Valid, string Email)> ValidateInvitationTokenAsync(string token)
        {
            var pendingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.ResetToken == token &&
                    !u.IsActive &&
                    u.TokenExpiration > DateTime.UtcNow);

            if (pendingUser == null)
                return (false, null);

            return (true, pendingUser.Email);
        }

       
        
        public async Task<(bool Success, string Message)> AcceptInvitationAsync(
            AcceptInvitationDto dto)
        {
            // 1. Retrouver le User fantôme
            var pendingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.ResetToken == dto.Token &&
                    !u.IsActive &&
                    u.TokenExpiration > DateTime.UtcNow);

            if (pendingUser == null)
                return (false, "Lien invalide ou expiré. Demandez une nouvelle invitation.");

            // 2. Compléter le profil
            pendingUser.LastName = dto.LastName;
            pendingUser.FirstName = dto.FirstName;
            pendingUser.PhoneNumber = dto.PhoneNumber ?? "";
            pendingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);

            // 3. Activer le compte et effacer le token
            pendingUser.IsActive = true;
            pendingUser.ResetToken = null;
            pendingUser.TokenExpiration = null;
            pendingUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (true, "Compte créé avec succès. Vous pouvez maintenant vous connecter.");
        }

       
        private string BuildInvitationEmail(User occupant, string link)
        {
            return $@"
            <html>
            <body style='font-family:Arial,sans-serif;background:#f4f4f4;padding:30px;'>
              <div style='max-width:600px;margin:auto;background:white;border-radius:10px;
                          padding:30px;box-shadow:0 2px 10px rgba(0,0,0,0.1);'>

                <div style='text-align:center;margin-bottom:20px;'>
                  <h2 style='color:#e74c3c;margin:0;'>🔥 Système Anti-Incendie Intelligent</h2>
                </div>

                <p style='font-size:16px;'>Bonjour,</p>
                <p style='font-size:15px;'>
                  <strong>{occupant.FirstName} {occupant.LastName}</strong> vous invite à rejoindre 
                  son système de surveillance anti-incendie pour protéger sa maison.
                </p>

                <div style='background:#fff5f5;border-left:4px solid #e74c3c;
                            padding:15px;margin:20px 0;border-radius:4px;'>
                  <p style='margin:0;font-size:14px;'>
                    ✅ Supervision en temps réel de la maison<br/>
                    🔔 Réception des alertes en cas de danger<br/>
                    📊 Accès à l'historique des incidents
                  </p>
                </div>

                <div style='text-align:center;margin:30px 0;'>
                  <a href='{link}'
                     style='background-color:#e74c3c;color:white;padding:14px 32px;
                            text-decoration:none;border-radius:6px;font-size:16px;
                            font-weight:bold;display:inline-block;'>
                    Créer mon compte →
                  </a>
                </div>

                <p style='color:#999;font-size:12px;text-align:center;margin-top:30px;'>
                  Ce lien est valable <strong>48 heures</strong>.<br/>
                  Si vous n'attendiez pas cette invitation, ignorez simplement cet email.
                </p>
              </div>
            </body>
            </html>";
        }
    }
}
