using Raven.Client.Documents;

namespace Subway.Mvp.Application.Abstractions;

public interface IDocumentStoreContainer
{
    IDocumentStore Store { get; }
}
