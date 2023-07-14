﻿using MediatR;
using NotSoSimpleEcommerce.Main.Domain.Commands;
using NotSoSimpleEcommerce.Main.Domain.Models;
using NotSoSimpleEcommerce.Repositories.Contracts;

namespace NotSoSimpleEcommerce.Main.Domain.CommandHandlers
{
    public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IDeleteEntityRepository<ProductEntity> _deleteEntityRepository;
        private readonly IReadEntityRepository<ProductEntity> _readRepository;

        public DeleteProductCommandHandler
        (
            IDeleteEntityRepository<ProductEntity> deleteEntityRepository,
            IReadEntityRepository<ProductEntity> readRepository
        )
        {
            _deleteEntityRepository = deleteEntityRepository ?? throw new ArgumentNullException(nameof(deleteEntityRepository));
            _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));

        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            //TODO Remove validation from controller
            var entity = await _readRepository.GetByIdAsync(request.Id, cancellationToken);
            if (entity is not null)
                await _deleteEntityRepository.ExecuteAsync(entity, cancellationToken);
        }
    }
}