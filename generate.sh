#!/usr/bin/env bash

# Copyright 2026 eBay Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

set -euo pipefail

echo "========================================="
echo "Communication SDK - Regeneration"
echo "========================================="

# Restore dotnet tools
echo "Restoring .NET tools..."
dotnet tool restore

# Regenerate Kiota clients
echo ""
echo "Regenerating Feedback API client..."
dotnet kiota generate \
  -l csharp \
  -d specs/openapi/feedback.yaml \
  -c FeedbackApiClient \
  -n EBay.Communication.Generated.Feedback \
  -o EBay.Communication/Generated/Feedback \
  --ebc --tam internal --co

echo ""
echo "Regenerating Message API client..."
dotnet kiota generate \
  -l csharp \
  -d specs/openapi/message.yaml \
  -c MessageApiClient \
  -n EBay.Communication.Generated.Message \
  -o EBay.Communication/Generated/Message \
  --ebc --tam internal --co

echo ""
echo "Regenerating Notification API client..."
dotnet kiota generate \
  -l csharp \
  -d specs/openapi/notification.yaml \
  -c NotificationApiClient \
  -n EBay.Communication.Generated.Notification \
  -o EBay.Communication/Generated/Notification \
  --ebc --tam internal --co

echo ""
echo "Regenerating Negotiation API client..."
dotnet kiota generate \
  -l csharp \
  -d specs/openapi/negotiation.yaml \
  -c NegotiationApiClient \
  -n EBay.Communication.Generated.Negotiation \
  -o EBay.Communication/Generated/Negotiation \
  --ebc --tam internal --co

echo "Adding license headers to generated files..."
./add-license-headers.sh ./EBay.Communication/Generated

# Build the project
echo ""
echo "Building the SDK..."
DOTNET_ROLL_FORWARD=LatestMajor dotnet build EBay.Communication/EBay.Communication.csproj

echo ""
echo "========================================="
echo "Done! SDK regenerated successfully."
echo "========================================="
