using Printpress.Domain.Entities;

namespace Printpress.Application
{
    internal sealed class ServiceService(IUnitOfWork _unitOfWork, ServiceMapper serviceMapper) : IServiceService
    {

        public async Task<ServiceDto> AddAsync(ServiceUpsertDto payload, string userId)
        {
            // Make validation

            Service service = serviceMapper.MapFromDestinationToSource(payload);

            await _unitOfWork.ServiceRepository.AddAsync(service);

            await _unitOfWork.SaveChangesAsync(userId);

            return serviceMapper.MapFromSourceToDestination(service);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            Service service = await _unitOfWork.ServiceRepository.FindAsync(id);

            if (service is null) throw new ValidationExeption("Service not found");

            _unitOfWork.ServiceRepository.Remove(service);
            await _unitOfWork.SaveChangesAsync(userId);
        }

        public async Task<List<ServiceDto>> GetAll()
        {
            List<Service> services = await _unitOfWork.ServiceRepository.AllAsync();

            return serviceMapper.MapFromSourceToDestination(services);
        }

        public async Task<ServiceDto> GetById(int id)
        {
            Service service = await _unitOfWork.ServiceRepository.FindAsync(id);

            if (service is null) throw new ValidationExeption("Service not found");

            return serviceMapper.MapFromSourceToDestination(service);
        }

        public async Task<ServiceDto> UpdateAsync(int id, ServiceUpsertDto payload, string userId)
        {
            // Make validation

            if (!_unitOfWork.ServiceRepository.Any(x => x.Id == id))
            {
                throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));
            }

            var client = _unitOfWork.ServiceRepository.Update(serviceMapper.MapFromDestinationToSource(id, payload));


            await _unitOfWork.SaveChangesAsync(userId);

            return serviceMapper.MapFromSourceToDestination(client);
        }
    }
}
