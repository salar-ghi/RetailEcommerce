﻿namespace Application.DTOs;

public class AuthDto
{

}


public class SignupDto
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}

public class LoginDto
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}

public class AddUserDto
{
    public string PhoneNumber { get; set; }
    public List<string> Roles { get; set; }
}   