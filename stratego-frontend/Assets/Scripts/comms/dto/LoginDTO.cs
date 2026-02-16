
using System;

[Serializable]
public class LoginDTO
{
	public string username;
	public string password;

	public LoginDTO(string username, string password)
	{
		this.username = username;
		this.password = password;
	}
}
