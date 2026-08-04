using FluentValidation;
using tuan3.DTO;

namespace tuan3.Validators
{
    public class UpdateStudentValidator:AbstractValidator<UpdateStudentDto>
    {
        public UpdateStudentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("ten khong duoc de trong")
                .MinimumLength(30).WithMessage("ki tu toi thieu 30 ki tu");

            RuleFor(x => x.Age)
                .InclusiveBetween(19, 23).WithMessage("do tuoi cua sinh vien pahi tu 19 den 23");




        }

    }
}
