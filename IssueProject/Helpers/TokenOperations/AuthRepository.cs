using IssueProject.Entity;
using IssueProject.Entity.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IssueProject.Helpers.TokenOperations
{
    public class AuthRepository : IAuthRepository
    {
        private _2Mes_ConceptualContext _context;
        public AuthRepository(_2Mes_ConceptualContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string userName, string password)
        {
            var user = await _context.Users.Include(x =>x.Role).
                FirstOrDefaultAsync(x => x.FullName == userName );
            if (user == null)
            {
                return null;
            }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] userPasswordHash, byte[] userPasswordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(userPasswordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != userPasswordHash[i])
                    {
                        return false;
                    }
                }
                return true;

            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            if (await _context.Users.AnyAsync(x => x.FullName == userName))
            {
                return true;
            }
            return false;

        }
    }
}