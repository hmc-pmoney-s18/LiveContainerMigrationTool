using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UnitTestProject1
{
    public static class Utilities
    {
        public static IEnumerable<string> GenerateDocumentsWithRandomIdAndPk(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return JsonConvert.SerializeObject(new TemplateDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true));
            }
        }
    }
}
