using FluentValidation;
using ShopApi.Dtos;

namespace ShopApi.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            // Email 規則
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email 不能為空")
                .EmailAddress().WithMessage("這不是有效的 Email 格式");

            // 密碼規則
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("密碼不能為空")
                .MinimumLength(6).WithMessage("密碼至少要 6 個字")
                .Matches("[A-Z]").WithMessage("密碼必須包含至少一個大寫字母")
                .Matches("[a-z]").WithMessage("密碼必須包含至少一個小寫字母")
                .Matches("[0-9]").WithMessage("密碼必須包含至少一個數字");
        }
    }
}