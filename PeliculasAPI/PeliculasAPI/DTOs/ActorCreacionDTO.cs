﻿using PeliculasAPI.Validaciones;

namespace PeliculasAPI.DTOs
{
    public class ActorCreacionDTO : ActorPatchDTO
    {
        [PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }
    }
}
