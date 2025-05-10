using MediatR;
using UserService.Entities;

namespace UserService
{
    public class GetAlluserQuery : IRequest<List<User>>
    {
    }
}
