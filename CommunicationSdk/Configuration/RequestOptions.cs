// Copyright 2026 eBay Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System;
using System.Collections.Generic;

namespace EBay.Communication.Configuration;

/// <summary>
/// Per-call request options for custom headers and other request configuration.
/// </summary>
public sealed class RequestOptions
{
    /// <summary>
    /// Custom HTTP headers to include with the request.
    /// Headers set here override DefaultRequestOptions headers from the configuration.
    /// </summary>
    public IDictionary<string, string> Headers { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
