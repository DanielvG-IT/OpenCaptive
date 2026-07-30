import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Emits .next/standalone with a self-contained server.js and only the traced subset of
  // node_modules. The Dockerfile depends on this - without it the runtime image would have
  // to carry the full dependency tree.
  output: "standalone",
};

export default nextConfig;
