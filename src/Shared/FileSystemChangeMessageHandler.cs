// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.FileSystem;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Messaging;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Utils.Messaging
{
    public class FileSystemChangeMessageHandler : MessageHandlerBase
    {
        private static HashSet<string> _messagetypes = new HashSet<string>() { MessageTypes.FileSystemChangeInformation.Value };

        public FileSystemChangeMessageHandler(ILogger logger)
            : base(logger)
        {

        }

        public override ISet<string> MessageTypesHandled => _messagetypes;

        protected override bool HandleMessageInternal(IMessageSender sender, Message message)
        {
            // What if the deserialization fails?
            FileSystemChangeInformation info = message.Payload.ToObject<FileSystemChangeInformation>();
            if (info == null)
            {
                Logger.Log(MessageStrings.InvalidFileSystemChangeMessage);
                Logger.Log(message.Payload.ToString());
            }

            Logger.Log($"{Environment.NewLine}\t\t{MessageStrings.StartFileSystemChangeToken}");
            switch (info.FileSystemChangeType)
            {
                case FileSystemChangeType.AddFile:
                    Logger.Log(string.Format(MessageStrings.AddFileMessage, info.FullPath));
                    Logger.Log(string.Format(MessageStrings.ContentsMessage, info.FileContents));
                    break;
                case FileSystemChangeType.EditFile:
                    Logger.Log(string.Format(MessageStrings.EditFileMessage, info.FullPath));
                    Logger.Log(string.Format(MessageStrings.ContentsMessage, info.FileContents));
                    break;
                case FileSystemChangeType.DeleteFile:
                    Logger.Log(string.Format(MessageStrings.DeleteFileMessage, info.FullPath));
                    break;
                case FileSystemChangeType.AddDirectory:
                    Logger.Log(string.Format(MessageStrings.AddDirectoryMessage, info.FullPath));
                    break;
                case FileSystemChangeType.RemoveDirectory:
                    Logger.Log(string.Format(MessageStrings.RemoveDirectoryMessage, info.FullPath));
                    break;
            }
            Logger.Log($"\t\t{MessageStrings.EndFileSystemChangeToken}{Environment.NewLine}");
            return true;
        }
    }
}
