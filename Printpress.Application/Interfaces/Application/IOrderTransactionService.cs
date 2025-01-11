﻿namespace Printpress.Application;

public interface IOrderTransactionService
{
    Task<OrderTransactionDto> AddAsync(OrderTransactionAddDto payload);
    Task<PagedList<OrderTransactionDto>> GetByPage(int pageNumber, int pageSize);

}