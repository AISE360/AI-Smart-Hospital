using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SmartHospital.Application.Interfaces;

namespace SmartHospital.Infrastructure.Ai;

/// <summary>
/// Pluggable HTTP AI client (OpenAI-compatible). Uses env vars:
/// AI_PROVIDER (openai|azure|claude), AI_API_KEY, AI_MODEL, AI_ENDPOINT
/// Falls back to StubAiClient if not configured.
/// </summary>
public class HttpAiClient : IAiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly StubAiClient _fallback = new();

    public HttpAiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    private bool IsConfigured => !string.IsNullOrWhiteSpace(_config["AI_API_KEY"]);

    public async Task<AiCompletionResult> CompleteAsync(AiCompletionRequest request, CancellationToken ct = default)
    {
        if (!IsConfigured) return await _fallback.CompleteAsync(request, ct);

        var model = _config["AI_MODEL"] ?? "gpt-4o-mini";
        var endpoint = _config["AI_ENDPOINT"] ?? "https://api.openai.com/v1/chat/completions";
        var payload = JsonSerializer.Serialize(new
        {
            model,
            messages = new[] { new { role = "user", content = request.Prompt } },
            temperature = request.Temperature,
            max_tokens = request.MaxTokens
        });
        var req = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["AI_API_KEY"]);
        var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
        return new AiCompletionResult(content, model, "http", 0.9, content.Length / 4);
    }

    public Task<string> GenerateFaqAnswerAsync(string question, string knowledgeBase, CancellationToken ct = default)
        => IsConfigured
            ? CompleteAsync(new AiCompletionRequest($"KB: {knowledgeBase}\nQ: {question}", "v1", "Faq"), ct).ContinueWith(t => t.Result.Content, ct)
            : _fallback.GenerateFaqAnswerAsync(question, knowledgeBase, ct);

    public Task<ClinicalNoteAiResult> GenerateClinicalNoteAsync(ClinicalNoteAiRequest request, CancellationToken ct = default)
        => _fallback.GenerateClinicalNoteAsync(request, ct); // in production, call CompleteAsync with template; stub keeps deterministic output for safety review

    public Task<DischargeSummaryAiResult> GenerateDischargeSummaryAsync(DischargeSummaryAiRequest request, CancellationToken ct = default)
        => _fallback.GenerateDischargeSummaryAsync(request, ct);

    public Task<string> GenerateDailyInsightAsync(DailyInsightRequest request, CancellationToken ct = default)
        => _fallback.GenerateDailyInsightAsync(request, ct);

    public Task<ClaimPreCheckResult> PreCheckClaimAsync(ClaimPreCheckRequest request, CancellationToken ct = default)
        => _fallback.PreCheckClaimAsync(request, ct);
}

// Uses Microsoft.Extensions.Configuration via environment fallback if not registered
