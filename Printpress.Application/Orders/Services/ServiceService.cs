using Printpress.Domain;

namespace Printpress.Application
{
    internal sealed class ServiceService(IUnitOfWork _unitOfWork, ServiceMapper serviceMapper, IGuidGenerator _guidGenerator) : IServiceService
    {

        public async Task<ServiceDto> AddAsync(ServiceUpsertDto payload, string userId)
        {
            // Make validation

            Service service = serviceMapper.MapFromDestinationToSource(payload);
            service.Id = _guidGenerator.NewGuid();

            await _unitOfWork.ServiceRepository.AddAsync(service);

            await _unitOfWork.SaveChangesAsync(userId);

            return await GetById(service.Id);
        }

        public async Task DeleteAsync(Guid id, string userId)
        {
            Service service = await _unitOfWork.ServiceRepository.FindAsync(id);

            if (service is null) throw new ValidationExeption("Service not found");

            _unitOfWork.ServiceRepository.Remove(service);
            await _unitOfWork.SaveChangesAsync(userId);
        }

        public async Task<List<ServiceDto>> GetAll()
        {
            List<Service> services = await _unitOfWork.ServiceRepository.AllAsync(nameof(Service.ServiceCategory));

            return serviceMapper.MapFromSourceToDestination(services);
        }

        public async Task<ServiceDto> GetById(Guid id)
        {
            Service service = await _unitOfWork.ServiceRepository.FirstOrDefaultAsync(x => x.Id == id, false, nameof(Service.ServiceCategory));

            if (service is null) throw new ValidationExeption("Service not found");

            return serviceMapper.MapFromSourceToDestination(service);
        }

        public async Task<ServiceDto> UpdateAsync(Guid id, ServiceUpsertDto payload, string userId)
        {
            // Make validation

            if (!_unitOfWork.ServiceRepository.Any(x => x.Id == id))
            {
                throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));
            }

            _unitOfWork.ServiceRepository.Update(serviceMapper.MapFromDestinationToSource(id, payload));

            await _unitOfWork.SaveChangesAsync(userId);

            return await GetById(id);
        }
    }
}
