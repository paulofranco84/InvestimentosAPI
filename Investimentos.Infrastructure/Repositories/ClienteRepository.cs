using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Context;

namespace Investimentos.Infrastructure.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(AppDbContext context) : base(context)
    {
    }
}
