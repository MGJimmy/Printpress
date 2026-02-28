using MediatR;

namespace Identity.Service
{
    public class GetAlluserQuery : IRequest<List<User>>
    {
    }
}
