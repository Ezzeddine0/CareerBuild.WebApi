﻿using AbstractServices;
using System;
using AutoMapper;
using Domain.Entities.IdentityModule;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Dtos.IdentityModule.Login;
using Shared.Dtos.IdentityModule.Register;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
	public class AuthenticationServices(
		UserManager<AppUser> _userManager,
		IMapper _mapper,
		IConfiguration Configuration)
		: IAuthenticationServices
	{
		#region Helper Methods

		private async Task<string> CreateTokenAsync(AppUser user)
		{
			// Collect user claims
			var claims = new List<Claim>
			{
				new(ClaimTypes.Email, user.Email!),
				new(ClaimTypes.NameIdentifier, user.UserName!)
			};

			// Dynamically retrieve roles based on user type
			var roles = await _userManager.GetRolesAsync(user);

			// Add roles to claims
			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			// Retrieve JWT options from configuration
			var jwtOptions = Configuration.GetSection("JWToptions");
			var securityKey = jwtOptions["securityKey"];
			var issuer = jwtOptions["issuer"];
			var audience = jwtOptions["audience"];

			// Create signing credentials
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// Generate the token
			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: DateTime.UtcNow.AddHours(2),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private async Task<bool> RegisterAsync<TEntity, TRegister>(TEntity entity, TRegister registerDto)
			where TEntity : AppUser
			where TRegister : RegisterBaseDto
		{
			var result = await _userManager
				.CreateAsync(entity, registerDto.Password);

			if (!result.Succeeded)
			{
				var errors = result.Errors
					.Select(e => e.Description).ToList();
				throw new BadRequestException(errors);
			}

			return result.Succeeded;
		}

		private async Task<TLoggedIn> LoginHelper<TEntity, TLoggedIn>(LoginDto loginDto)
			where TEntity : AppUser
			where TLoggedIn : LoggedInBase
		{
			TEntity? user = await _userManager.FindByEmailAsync(loginDto.Email) as TEntity;

			if (user == null)
			{
				throw new UserNotFoundException(loginDto.Email);
			}

			var validPassword = await _userManager
				.CheckPasswordAsync(user, loginDto.Password);

			if (!validPassword)
			{
				throw new WrongLoginException();
			}

			var result = _mapper.Map<TLoggedIn>(user);
			result.Token = await CreateTokenAsync(user);

			return result;
		}

		#endregion

		public async Task<LoggedInUserDto> LoginRegularUserAsync(LoginDto loginDto)
		{
			return await LoginHelper<RegularUser, LoggedInUserDto>(loginDto);
		}

		public async Task<LoggedInCompanyDto> LoginCompanyAsync(LoginDto loginDto)
		{
			return await LoginHelper<CompanyUser, LoggedInCompanyDto>(loginDto);
		}

		public async Task<bool> RegisterCompanyUserAsync(RegisterCompanyDto registerDto)
		{
			// it's not possible to have null user as the function will throw exception
			var user = _mapper.Map<CompanyUser>(registerDto);

			return await RegisterAsync(user, registerDto);
		}

		public async Task<bool> RegisterRegularUserAsync(RegisterUserDto registerDto)
		{
			var user = _mapper.Map<RegularUser>(registerDto);

			return await RegisterAsync(user, registerDto);
		}

		public async Task<bool> DeleteUser(string? email)
		{
			if (email == null)
			{
				throw new Exception("No Email Is Provided");
			}

			var user = await _userManager.FindByIdAsync(email);

			if (user == null)
			{
				throw new UserNotFoundException(email);
			}

			var result = await _userManager.DeleteAsync(user);

			if (!result.Succeeded)
			{
				var errors = result.Errors
					.Select(e => e.Description).ToList();
				throw new BadRequestException(errors);
			}

			return result.Succeeded;
		}

		public async Task<bool> UpdatePassword(string? email, string currentPassword, string newPassword)
		{
			if (email == null)
			{
				throw new Exception("No Email Is Provided");
			}

			var user = await _userManager.FindByIdAsync(email);

			if (user == null)
			{
				throw new UserNotFoundException(email);
			}


			var result = await _userManager
				.ChangePasswordAsync(user, currentPassword, newPassword);

			if (!result.Succeeded)
			{
				var errors = result.Errors
					.Select(e => e.Description).ToList();
				throw new BadRequestException(errors);
			}

			return result.Succeeded;
		}
	}
}