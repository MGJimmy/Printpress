using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Entities;
using UserService.Presistance;

namespace UserService
{
    public class GetAlluserQueryHandler : IRequestHandler<GetAlluserQuery, List<User>>
    {
        private readonly UserDbContext _context;

        public GetAlluserQueryHandler(UserDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Handle(GetAlluserQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.ToListAsync(cancellationToken);
        }
    }
}