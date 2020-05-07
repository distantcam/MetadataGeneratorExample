using FluentValidation;
using TheProject.Entities;

namespace TheProject.Validation {
    public class UserValidator : AbstractValidator<User> {
        public UserValidator() {
            RuleFor(entity => entity.FirstName).NotNull();
            RuleFor(entity => entity.LastName);
            RuleFor(entity => entity.Email).EmailAddress();
            RuleFor(entity => entity.CreditCard).CreditCard();

        }
    }
}
