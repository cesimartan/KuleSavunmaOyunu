using System.Collections.Generic;
using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Core
{
    public interface ISaldirabilir
    {
        void Saldir(List<Dusman> dusmanlar);
    }
}
