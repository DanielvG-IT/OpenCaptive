import { createRoot } from "react-dom/client";
import { StrictMode } from "react";
import "./main.css";

const container = document.getElementById("root");
if (!container) throw new Error("Root container missing in index.html");

const root = createRoot(container);
root.render(
  <StrictMode>
    <div
      style={{
        fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, sans-serif",
        padding: 24,
      }}>
      <h1>Captive Portal — Prototype</h1>
      <p>
        This page reads query parameters from the browser (for example:{" "}
        <code>?clientId=...</code>) and can call the backend /authorize endpoint
        for debugging.
      </p>

      <section style={{ marginBottom: 12 }}>
        <h2>Query parameters (debug)</h2>
        <pre style={{ background: "#f5f5f5", padding: 12, borderRadius: 6 }}>
          {JSON.stringify(
            Object.fromEntries(new URLSearchParams(window.location.search)),
            null,
            2
          )}
        </pre>
      </section>

      <section style={{ marginBottom: 12 }}>
        <button
          onClick={() => {
            try {
              const params = Object.fromEntries(
                new URLSearchParams(window.location.search)
              );
              const macAddress = params.id ?? params.id ?? params.id;
              const out = document.getElementById("authorize-result");
              if (!macAddress) {
                if (out)
                  out.textContent = "No macAddress found in query params.";
                return;
              }

              if (out) out.textContent = "Calling /authorize...";
              fetch(
                `http://192.168.1.143:3000/authorize?macAddress=${encodeURIComponent(String(macAddress))}`
              )
                .then(async (r) => {
                  const text = await r.text();
                  let parsed;
                  try {
                    parsed = JSON.stringify(JSON.parse(text), null, 2);
                  } catch {
                    parsed = text;
                  }
                  if (out) out.textContent = `HTTP ${r.status}\n\n${parsed}`;
                })
                .catch((err) => {
                  if (out) out.textContent = `Fetch error: ${String(err)}`;
                });
            } catch (err) {
              const out = document.getElementById("authorize-result");
              if (out) out.textContent = `Error: ${String(err)}`;
            }
          }}
          style={{
            padding: "8px 12px",
            fontSize: 14,
            borderRadius: 6,
            cursor: "pointer",
            marginRight: 8,
          }}>
          Authorize (call /authorize)
        </button>

        <button
          onClick={() => {
            const params = new URLSearchParams(window.location.search);
            // convenience: open the authorize endpoint in a new tab (for manual testing)
            const id = params.get("id");
            if (!id) {
              const out = document.getElementById("authorize-result");
              if (out) out.textContent = "No id found in query params.";
              return;
            }
            window.open(
              `/authorize?clientId=${encodeURIComponent(id)}`,
              "_blank"
            );
          }}
          style={{
            padding: "8px 12px",
            fontSize: 14,
            borderRadius: 6,
            cursor: "pointer",
          }}>
          Open /authorize in new tab
        </button>
      </section>

      <section>
        <h2>Result</h2>
        <pre
          id="authorize-result"
          style={{
            background: "#111",
            color: "#0f0",
            padding: 12,
            borderRadius: 6,
          }}>
          Press "Authorize" to call the backend and show the response here.
        </pre>
      </section>

      <footer style={{ marginTop: 18, fontSize: 12, color: "#666" }}>
        Prototype — clientId and other parameters are taken from the browser
        query string for debugging.
      </footer>
    </div>
  </StrictMode>
);
