﻿namespace Application.Helper;

public class PasswordHasher : IPasswordHasher
{
    public PasswordHasher()
    {
        
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
