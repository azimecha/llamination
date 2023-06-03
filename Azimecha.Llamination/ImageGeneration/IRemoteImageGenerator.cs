using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.ImageGeneration {
    public interface IRemoteImageGenerator : IImageGenerator {
        IEnumerable<IRemoteModel> Models { get; }
    }

    public interface IRemoteModel {
        string Name { get; }
        string DisplayName { get; }
        byte[] Hash { get; }
        void Select();
    }
}
