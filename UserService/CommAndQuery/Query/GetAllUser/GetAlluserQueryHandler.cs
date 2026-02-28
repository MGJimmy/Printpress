using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Service
{
    public class GetAlluserQueryHandler : IRequestHandler<GetAlluserQuery, List<User>>
    {
        private readonly IdentityDbContext _context;

        public GetAlluserQueryHandler(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Handle(GetAlluserQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.ToListAsync(cancellationToken);
        }
    }
}