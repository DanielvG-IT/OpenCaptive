import axios from "axios";
import { Agent } from "https";

class UnifiApiClient {
  constructor({
    baseUrl,
    apiKey,
    insecure = true,
    timeout = 5000,
    logger = console,
  } = {}) {
    this.logger = logger;
    const headers = {
      "Content-Type": "application/json",
      Accept: "application/json",
    };
    if (apiKey) headers["X-API-KEY"] = `${apiKey}`;

    const normalizeBaseUrl = (u) => {
      if (!u) return u;
      // if it already contains a scheme (http:// or https://) leave it, otherwise default to https
      if (/^[a-zA-Z][a-zA-Z0-9+.-]*:\/\//.test(u)) {
        return u.replace(/\/+$/, "");
      }
      return `https://${u.replace(/\/+$/, "")}`;
    };

    const normalizedBaseUrl = normalizeBaseUrl(baseUrl);

    this.client = axios.create({
      baseURL: normalizedBaseUrl,
      timeout,
      headers,
      httpsAgent: new Agent({ rejectUnauthorized: !insecure }),
    });

    // Helper to avoid logging sensitive headers
    const redactHeaders = (h = {}) => {
      const copy = { ...h };
      if (copy["X-API-KEY"]) copy["X-API-KEY"] = "[REDACTED]";
      return copy;
    };

    // Safe stringify for logging
    const safeStringify = (obj, maxLen = 2000) => {
      try {
        const s = JSON.stringify(obj);
        return s.length > maxLen ? s.slice(0, maxLen) + "...(truncated)" : s;
      } catch {
        return String(obj);
      }
    };

    // Request logging + start time
    this.client.interceptors.request.use(
      (config) => {
        config.metadata = { startTime: Date.now() };
        try {
          this.logger.debug?.(
            "[UnifiApiClient] request",
            config.method?.toUpperCase(),
            config.url,
            "headers:",
            redactHeaders(config.headers),
            "params:",
            config.params ?? null,
            "data:",
            config.data ?? null
          );
        } catch (e) {
          this.logger.debug?.("[UnifiApiClient] request (logging failed)", e);
        }
        return config;
      },
      (error) => {
        this.logger.error?.("[UnifiApiClient] request error", error);
        return Promise.reject(error);
      }
    );

    // Response logging
    this.client.interceptors.response.use(
      (response) => {
        const duration = response.config?.metadata
          ? Date.now() - response.config.metadata.startTime
          : null;
        try {
          this.logger.debug?.(
            "[UnifiApiClient] response",
            response.status,
            response.config?.url,
            "duration:",
            duration != null ? `${duration}ms` : "unknown",
            "data:",
            safeStringify(response.data)
          );
        } catch (e) {
          this.logger.debug?.("[UnifiApiClient] response (logging failed)", e);
        }
        return response;
      },
      (error) => {
        const resp = error.response;
        const duration = error.config?.metadata
          ? Date.now() - error.config.metadata.startTime
          : null;
        if (resp) {
          this.logger.error?.(
            "[UnifiApiClient] response error",
            resp.status,
            error.config?.url,
            "duration:",
            duration != null ? `${duration}ms` : "unknown",
            "data:",
            safeStringify(resp.data)
          );
        } else {
          this.logger.error?.(
            "[UnifiApiClient] network/axios error",
            error.message
          );
        }
        return Promise.reject(error);
      }
    );
  }

  async getSites() {
    try {
      this.logger.debug?.("[UnifiApiClient] getSites - calling API");
      const res = await this.client.get("/proxy/network/integration/v1/sites");

      if (res.status !== 200) {
        throw new Error(`Unifi API error ${res.status}`);
      }

      const result = (res.data && (res.data.data ?? res.data)) || [];
      this.logger.debug?.(
        "[UnifiApiClient] getSites - result count",
        Array.isArray(result) ? result.length : 1
      );
      return result;
    } catch (err) {
      this.logger.error?.(
        "[UnifiApiClient] getSites - error",
        err?.message ?? err
      );
      if (err.response) {
        const e = new Error(`Unifi API error ${err.response.status}`);
        e.status = err.response.status;
        e.data = err.response.data;
        throw e;
      }
      throw err;
    }
  }

  getClientIdByMac(siteId, macAddress) {
    if (!siteId || !macAddress) {
      this.logger.error?.(
        "[UnifiApiClient] getClientIdByMac - missing params",
        { siteId, macAddress }
      );
      throw new Error("siteId and macAddress are required");
    }

    // Build the query string ourselves and URI-encode the whole filter so it isn't double-encoded by axios
    const basePath = `/proxy/network/integration/v1/sites/${encodeURIComponent(
      siteId
    )}/clients`;
    const filter = `macAddress.eq('${macAddress}')`;
    const url = `${basePath}?filter=${encodeURIComponent(filter)}`;

    return this.client.get(url).then((res) => {
      if (res.status !== 200) {
        throw new Error(`Unifi API error ${res.status}`);
      }
      const raw = (res.data && (res.data.data ?? res.data)) || [];
      const clients = Array.isArray(raw) ? raw : [raw];

      // Normalize comparison to lowercase and tolerate different mac field names
      const targetMac = (macAddress || "").toLowerCase();
      const match = clients.find((c) => {
        const macs = [
          c.mac,
          c.macAddress,
          c.client_mac,
          c.clientMac,
          c.device_mac,
        ].filter(Boolean);
        return macs.some((m) => String(m).toLowerCase() === targetMac);
      });

      if (!match) {
        throw new Error("Client not found");
      }
      return match.id;
    });
  }

  async authorizeGuest(siteId, clientId) {
    if (!siteId || !clientId) {
      this.logger.error?.(
        "[UnifiApiClient] authorizeGuest - missing parameters",
        { siteId, clientId }
      );
      throw new Error("siteId and clientId are required");
    }

    const path = `/proxy/network/integration/v1/sites/${encodeURIComponent(siteId)}/clients/${encodeURIComponent(clientId)}/actions`;
    const actionPayload = {
      action: "AUTHORIZE_GUEST_ACCESS",
      timeLimitMinutes: 1,
      dataUsageLimitMBytes: 1,
      rxRateLimitKbps: 2,
      txRateLimitKbps: 2,
    };

    try {
      this.logger.debug?.(
        "[UnifiApiClient] authorizeGuest - calling API",
        path,
        "payload:",
        actionPayload
      );
      const res = await this.client.post(path, actionPayload);

      if (res.status !== 200) {
        throw new Error(`Unifi API error ${res.status}`);
      }

      const { action, revokedAuthorization, grantedAuthorization } =
        res.data || {};
      const normalized = {
        action,
        revoked: revokedAuthorization || null,
        granted: grantedAuthorization || null,
      };
      this.logger.debug?.(
        "[UnifiApiClient] authorizeGuest - success",
        normalized
      );
      return normalized;
    } catch (err) {
      this.logger.error?.(
        "[UnifiApiClient] authorizeGuest - error",
        err?.message ?? err
      );
      if (err.response) {
        const e = new Error(`Unifi API error ${err.response.status}`);
        e.status = err.response.status;
        e.data = err.response.data;
        throw e;
      }
      throw err;
    }
  }
}

export default UnifiApiClient;
