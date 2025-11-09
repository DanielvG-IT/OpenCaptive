import cors from "cors";
import express from "express";
import bodyParser from "body-parser";
import unifi from "./unifi.js";
import dotenv from "dotenv";

// Load .env when present. It's okay if it doesn't exist in production.
dotenv.config();

const app = express();
const PORT = process.env.PORT || 3000;

// Allow turning debug on/off via env var, default true for local dev
const debugEnabled = process.env.DEBUG !== "false";

const debugLog = (...args) => {
  if (debugEnabled) {
    console.debug(new Date().toISOString(), ...args);
  }
};

debugLog("Debug logging enabled");

// Read Unifi configuration from environment (use a .env file for local dev)
const UNIFI_BASE_URL = process.env.UNIFI_BASE_URL || "192.168.1.1";
const UNIFI_API_KEY = process.env.UNIFI_API_KEY || undefined;
const SITE_ID =
  process.env.UNIFI_SITE_ID || "88f7af54-98f8-306a-a1c7-c9349722b1f6";

if (!UNIFI_API_KEY) {
  debugLog(
    "Warning: UNIFI_API_KEY is not set. This should be provided via .env in development or env vars in production."
  );
}

const unifiClient = new unifi({
  baseUrl: UNIFI_BASE_URL,
  apiKey: UNIFI_API_KEY,
});

debugLog("Unifi client created", { baseUrl: UNIFI_BASE_URL });

// Middleware
app.use(cors());
app.use(bodyParser.json());

// Request logging middleware
app.use((req, res, next) => {
  const start = Date.now();
  debugLog("Incoming request", {
    method: req.method,
    url: req.originalUrl,
    query: req.query,
    body: req.body,
  });

  res.on("finish", () => {
    const duration = Date.now() - start;
    debugLog("Request completed", {
      method: req.method,
      url: req.originalUrl,
      status: res.statusCode,
      durationMs: duration,
    });
  });

  next();
});

// Routes
app.get("/sites", (req, res) => {
  debugLog("GET /sites - fetching sites");
  unifiClient
    .getSites()
    .then((sites) => {
      debugLog("GET /sites - success", {
        count: Array.isArray(sites) ? sites.length : undefined,
      });
      res.json(sites);
    })
    .catch((err) => {
      console.error(err);
      debugLog("GET /sites - error", {
        message: err.message,
        status: err.status,
      });
      res.status(err.status || 500).json({ error: err.message });
    });
});

app.get("/authorize", async (req, res) => {
  const { macAddress } = req.query;
  debugLog("GET /authorize - params", { macAddress });

  if (!macAddress || typeof macAddress !== "string") {
    debugLog("GET /authorize - missing or invalid macAddress");
    return res
      .status(400)
      .json({ error: "Missing or invalid 'macAddress' query parameter" });
  }

  debugLog("GET /authorize - fetching client ID for MAC", { macAddress });
  const siteId = "88f7af54-98f8-306a-a1c7-c9349722b1f6"; // Replace with actual siteId if needed

  try {
    // getClientIdByMac returns a Promise that resolves to the client id string
    const clientId = await unifiClient.getClientIdByMac(siteId, macAddress);

    if (!clientId) {
      debugLog("GET /authorize - client ID not found for MAC", { macAddress });
      return res
        .status(404)
        .json({ error: `Client with MAC ${macAddress} not found` });
    }

    debugLog("GET /authorize - authorizing guest", { clientId, siteId });

    const result = await unifiClient.authorizeGuest(siteId, clientId);
    debugLog("GET /authorize - success", { result });
    return res.json(result);
  } catch (err) {
    // If getClientIdByMac throws "Client not found" keep it as 404, otherwise use err.status if present
    console.error(err);
    const status = err.message === "Client not found" ? 404 : err.status || 500;
    debugLog("GET /authorize - error", {
      message: err.message,
      status: status,
    });
    return res.status(status).json({ error: err.message });
  }
});

// Start server
app.listen(PORT, "0.0.0.0", () => {
  console.log(`Server is running on http://0.0.0.0:${PORT}`);
  debugLog("Server started", { port: PORT, host: "0.0.0.0" });
});
