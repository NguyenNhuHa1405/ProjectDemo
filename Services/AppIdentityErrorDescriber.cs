using Microsoft.AspNetCore.Identity;

namespace App.Services
{
  public class AppIdentityErrorDescriber : IdentityErrorDescriber
  {
      public override IdentityError ConcurrencyFailure()
      {
          return base.ConcurrencyFailure();
      }

      public override IdentityError DefaultError()
      {
        return base.DefaultError();
      }

      public override IdentityError DuplicateEmail(string email)
      {
        return base.DuplicateEmail(email);
      }

      public override IdentityError DuplicateRoleName(string role)
      {
          var er = base.DuplicateRoleName(role);

          return new IdentityError()
          { 
              Code  = er.Code,
              Description = $"Role có tên {role} bị trùng"
          };

      }

      public override IdentityError DuplicateUserName(string userName)
      {
        return base.DuplicateUserName(userName);
      }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
      public override bool Equals(object obj)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
      {
        return base.Equals(obj);
      }

      public override int GetHashCode()
      {
        return base.GetHashCode();
      }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override IdentityError InvalidEmail(string email)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {
        return base.InvalidEmail(email);
      }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override IdentityError InvalidRoleName(string role)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {
        return base.InvalidRoleName(role);
      }

      public override IdentityError InvalidToken()
      {
        return base.InvalidToken();
      }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
      public override IdentityError InvalidUserName(string userName)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
      {
        return base.InvalidUserName(userName);
      }

      public override IdentityError LoginAlreadyAssociated()
      {
        return base.LoginAlreadyAssociated();
      }

      public override IdentityError PasswordMismatch()
      {
        return base.PasswordMismatch();
      }

      public override IdentityError PasswordRequiresDigit()
      {
        return base.PasswordRequiresDigit();
      }

      public override IdentityError PasswordRequiresLower()
      {
        return base.PasswordRequiresLower();
      }

      public override IdentityError PasswordRequiresNonAlphanumeric()
      {
        return base.PasswordRequiresNonAlphanumeric();
      }

      public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
      {
        return base.PasswordRequiresUniqueChars(uniqueChars);
      }

      public override IdentityError PasswordRequiresUpper()
      {
        return base.PasswordRequiresUpper();
      }

      public override IdentityError PasswordTooShort(int length)
      {
        return base.PasswordTooShort(length);
      }

      public override IdentityError RecoveryCodeRedemptionFailed()
      {
        return base.RecoveryCodeRedemptionFailed();
      }

      public override string ToString()
      {
#pragma warning disable CS8603 // Possible null reference return.
            return base.ToString();
#pragma warning restore CS8603 // Possible null reference return.
        }

      public override IdentityError UserAlreadyHasPassword()
      {
        return base.UserAlreadyHasPassword();
      }

      public override IdentityError UserAlreadyInRole(string role)
      {
        return base.UserAlreadyInRole(role);
      }

      public override IdentityError UserLockoutNotEnabled()
      {
        return base.UserLockoutNotEnabled();
      }

      public override IdentityError UserNotInRole(string role)
      {
        return base.UserNotInRole(role);
      }
  }
}