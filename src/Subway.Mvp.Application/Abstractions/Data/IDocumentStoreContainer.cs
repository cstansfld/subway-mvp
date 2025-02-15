using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Subway.Mvp.Application.Abstractions.Data;

public interface IDocumentStoreContainer
{
    IDocumentStore Store { get; }
}
