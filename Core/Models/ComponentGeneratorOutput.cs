﻿using System.Collections.Generic;

namespace Core.Models
{
    public class ComponentGeneratorOutput
    {
        public string Body { get; set; }

        public string CsFilePath { get; set; }
    }
    public class ComponentOutputComparer : IEqualityComparer<ComponentGeneratorOutput>
    {
        public bool Equals(ComponentGeneratorOutput x, ComponentGeneratorOutput y)
        {
            return x.CsFilePath == y.CsFilePath;
        }

        public int GetHashCode(ComponentGeneratorOutput obj)
        {
            return obj.GetHashCode();
        }
    }
}
