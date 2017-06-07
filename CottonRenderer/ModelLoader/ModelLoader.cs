using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CottonRenderer.ModelLoader
{
    public abstract class ModelLoader
    {
        public ModelLoader()
        {

        }
        public abstract Model[] LoadModelFile(string fileName);
        public abstract Task<Model[]> LoadModelFileAsync(string fileName);
    }
}
