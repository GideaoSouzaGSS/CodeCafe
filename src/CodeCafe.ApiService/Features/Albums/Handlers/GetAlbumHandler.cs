using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.ApiService.Features.Albums.Queries;
using CodeCafe.Domain.Entities;
using CodeCafe.Data;
using CodeCafe.Application.Interfaces.Services;

namespace CodeCafe.ApiService.Features.Albums.Handlers;

public class GetAlbumHandler : IRequestHandler<GetAlbumQuery, Album>
{
    private readonly AppDbContext _context;
    private readonly IBlobService _blobService;

    public GetAlbumHandler(AppDbContext context, IBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<Album> Handle(GetAlbumQuery request, CancellationToken cancellationToken)
    {
        // Buscar o álbum e suas fotos no banco de dados
        var album = await _context.Albums
            .Include(a => a.Photos)
            .FirstOrDefaultAsync(a => a.Id == request.AlbumId, cancellationToken);

        if (album == null)
            throw new KeyNotFoundException("Album not found.");

        // Baixar os arquivos das fotos do Blob Storage
        foreach (var photo in album.Photos)
        {
            try
            { 
                var stream = await _blobService.DownloadFilePrivateAsync(photo.Url);   
                // Aqui você pode processar o stream, se necessário.
                // Por exemplo, você pode retornar o stream ou salvar localmente.
                // Neste caso, vamos apenas indicar que o arquivo foi baixado com sucesso.
                photo.Url = $"File {photo.Url} downloaded successfully.";
            }
            catch (FileNotFoundException ex)
            {
                // Se o arquivo não for encontrado, você pode definir uma mensagem padrão ou lidar de outra forma.
                // Aqui, vamos apenas definir a URL como uma mensagem de erro.
                photo.Url = $"File not found: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Lidar com outras exceções conforme necessário
                photo.Url = $"Error downloading file: {ex.Message}";
            }
        }

        return album;
    }
}