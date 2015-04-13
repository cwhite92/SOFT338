using SOFT338.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Metadata;
using System.Web.ModelBinding;

namespace SOFT338.Controllers
{
    public class BaseController : ApiController
    {
        protected ApiDbContext db { get; set; }

        public BaseController()
        {
            this.db = new ApiDbContext();
        }

        /// <summary>
        /// Disposes of the database context at the end of a request cycle.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.db.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Revalidates the model passed as the first parameter.
        /// </summary>
        /// <param name="model">The model instance you wish to validate.</param>
        /// <returns></returns>
        protected internal bool TryValidateModel(object model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            System.Web.ModelBinding.ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            var t = new ModelBindingExecutionContext(new HttpContextWrapper(HttpContext.Current), new System.Web.ModelBinding.ModelStateDictionary());

            foreach (ModelValidationResult validationResult in ModelValidator.GetModelValidator(metadata, t).Validate(null))
            {
                ModelState.AddModelError(validationResult.MemberName, validationResult.Message);
            }

            return ModelState.IsValid;
        }
    }
}
