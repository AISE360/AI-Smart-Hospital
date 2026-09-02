namespace SmartHospital.Application.Interfaces;

public interface ISttProvider
{
    Task<TranscriptionResult> TranscribeAsync(Stream audioStream, string languageCode = "en-IN", CancellationToken ct = default);
}

public record TranscriptionResult(string Transcript, double Confidence, string Language, TimeSpan Duration);
