using Altec.Api.Domain.NiceLabel;

namespace Altec.Api.Services.NiceLabel;

public class NiceLabelService : INiceLabelService
{
    private readonly NiceLabelEngine _niceLabelEngine;

    public NiceLabelService(NiceLabelEngine niceLabelEngine)
    {
        _niceLabelEngine = niceLabelEngine;
    }
    
    public void PrintLabel(IFormFile label)
    {
        using var stream = label.OpenReadStream();
        var variables = _niceLabelEngine.GetVariables(stream);
    }
}