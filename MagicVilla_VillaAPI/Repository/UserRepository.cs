﻿using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private string _secretKey;

        public UserRepository(ApplicationDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;

            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(user => user.UserName == username);
            if (user == null)
            {
                return false;
            }

            return true;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            // SQL Query sent to SQL server is case-insensitive -> all string type is case-insensitve
            // 1st solution AsEnumerable() -> not good
            // 2nd collation: https://learn.microsoft.com/en-us/ef/core/miscellaneous/collations-and-case-sensitivity
            var user = _db.LocalUsers.AsEnumerable().FirstOrDefault(user => user.UserName == loginRequestDTO.UserName && user.Password == loginRequestDTO.Password);
            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = string.Empty,
                    User = null
                };
            }

            // 42, If a user was found generate JWT token. To gen. a JWT token, we need a SECRET KEY. Using that secret key, our token will be ENCRYPTED.
            // 42, That secret key will be use to validate if that token is valid or not, since everyone could gen. a Token.
            // 42, Secret key will be used to authenticate that whether the token was generated by OUR API, because no one else knows about that secret. Only the application has that secret key.
            var tokenHandler = new JwtSecurityTokenHandler();
            // 42, Encode secret key in byte array rather than string
            var key = Encoding.ASCII.GetBytes(_secretKey);
            // 42, Configure token descriptor
            // 42, Token descriptor contains everything like what are all the claims in a token. Claims will basically identify that this is the name of the user, this is the role that  you have and there are custom claims that you can add. But we also have built-in default claims for userID, name, role and much more.
            // 42, Along with that, we can define how long the token will be valid for, so we will have explaination there. And then, we will also have the sign in credentials where we will be adding the sign in credentials using the key that we added right here.
            var tokenDescriptor = new SecurityTokenDescriptor()
            { 
                // 42, Claims are payload data
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };  // What a token should contain, how to encrypt, when does it expire, and other...

            // 42, Gen. token - not serialized -> use WriteToken() to serialize SecurityToken
            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                // typeof(Token) == string
                Token = tokenHandler.WriteToken(token),
                User = user
            };

            // 42, If any Http Endpoint require authentication, Token must be passed in the request

            return loginResponseDTO;
        }

        public async Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            LocalUser user = _mapper.Map<LocalUser>(registerationRequestDTO);

            _db.LocalUsers.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }
    }
}