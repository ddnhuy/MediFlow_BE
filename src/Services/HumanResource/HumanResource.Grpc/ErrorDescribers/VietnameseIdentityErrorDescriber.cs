namespace HumanResource.Grpc.ErrorDescribers
{
    public class VietnameseIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
            => new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"Tên đăng nhập '{userName}' đã được sử dụng."
            };

        public override IdentityError DuplicateEmail(string email)
            => new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"Email '{email}' đã được sử dụng."
            };

        public override IdentityError InvalidUserName(string? userName)
            => new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = $"Tên đăng nhập '{userName}' không hợp lệ."
            };

        public override IdentityError InvalidEmail(string? email)
            => new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = $"Email '{email}' không hợp lệ."
            };

        public override IdentityError PasswordTooShort(int length)
            => new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"Mật khẩu phải có ít nhất {length} ký tự."
            };

        public override IdentityError PasswordRequiresDigit()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Mật khẩu phải chứa ít nhất một chữ số."
            };

        public override IdentityError PasswordRequiresLower()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Mật khẩu phải chứa ít nhất một chữ thường."
            };

        public override IdentityError PasswordRequiresUpper()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "Mật khẩu phải chứa ít nhất một chữ hoa."
            };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Mật khẩu phải chứa ít nhất một ký tự đặc biệt."
            };
    }
}
