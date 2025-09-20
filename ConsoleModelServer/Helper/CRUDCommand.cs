using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
	public class CRUDCommand
	{
        protected ModelRepository modelRepository;
        protected Model model;

        public CRUDCommand(ModelRepository repo, Model model)
        {
            this.modelRepository = repo;
            this.model = model;
        }
    }
}
