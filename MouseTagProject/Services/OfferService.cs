using DocumentFormat.OpenXml.Packaging;
using MouseTagProject.Models;


namespace MouseTagProject.Services
{
    public class OfferService
    {
        public async Task<byte[]> CreateOffer(Candidate candidate)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = Path.Combine(baseDirectory, "Templates", "OfferTemplate.docx");

            byte[] byteArray = File.ReadAllBytes(filePath);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, (int)byteArray.Length);
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(stream, true))
                {
                    var body = wordDocument.MainDocumentPart?.Document.Body;
                    if (wordDocument.MainDocumentPart != null)
                        if (wordDocument.MainDocumentPart.Document.Body != null)
                            wordDocument.MainDocumentPart.Document.Body.InnerXml = body?.InnerXml
                                .Replace("#VardasPavarde", $"{candidate.Name} {candidate.Surname}")
                                .Replace("Data", DateTime.Now.ToShortDateString()) ?? string.Empty;
                    wordDocument.Close();

                    return stream.ToArray();
                }
            }
        }
    }
}
