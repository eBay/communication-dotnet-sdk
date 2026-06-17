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
  -d specs/commerce_feedback_v1_beta_oas3.yaml \
  -c FeedbackApiClient \
  -n Communication.Generated.Feedback \
  -o CommunicationSdk/Generated/Feedback \
  --ebc --tam internal --co

echo ""
echo "Regenerating Message API client..."
dotnet kiota generate \
  -l csharp \
  -d specs/commerce_message_v1_oas3.yaml \
  -c MessageApiClient \
  -n Communication.Generated.Message \
  -o CommunicationSdk/Generated/Message \
  --ebc --tam internal --co

echo ""
echo "Regenerating Notification API client..."
dotnet kiota generate \
  -l csharp \
  -d specs/commerce_notification_v1_oas3.yaml \
  -c NotificationApiClient \
  -n Communication.Generated.Notification \
  -o CommunicationSdk/Generated/Notification \
  --ebc --tam internal --co

echo ""
echo "Regenerating Negotiation API client..."
dotnet kiota generate \
  -l csharp \
  -d specs/sell_negotiation_v1_oas3.yaml \
  -c NegotiationApiClient \
  -n Communication.Generated.Negotiation \
  -o CommunicationSdk/Generated/Negotiation \
  --ebc --tam internal --co

# Build the project
echo ""
echo "Building the SDK..."
dotnet build CommunicationSdk/CommunicationSdk.csproj

echo ""
echo "========================================="
echo "Done! SDK regenerated successfully."
echo "========================================="
