﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class ServiceService(IUnitOfWork _unitOfWork, ServiceMapper serviceMapper) : IServiceService
    {

        public async Task<ServiceDto> AddAsync(ServiceUpsertDto payload)
        {
            // Make validation

            Service service = await _unitOfWork.ServiceRepository.AddAsync(serviceMapper.MapFromDestinationToSource(payload));

            await _unitOfWork.SaveChangesAsync();

            return serviceMapper.MapFromSourceToDestination(service);
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
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

        public async Task<ServiceDto> UpdateAsync(int id, ServiceUpsertDto payload)
        {
            // Make validation

            if (!_unitOfWork.ServiceRepository.Any(x => x.Id == id))
            {
                throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));
            }

            var client = _unitOfWork.ServiceRepository.Update(serviceMapper.MapFromDestinationToSource(id, payload));


            await _unitOfWork.SaveChangesAsync();

            return serviceMapper.MapFromSourceToDestination(client);
        }
    }
}
