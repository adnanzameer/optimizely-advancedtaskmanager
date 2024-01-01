using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AdvancedTaskManager.Features.AdvancedTask;
using EPiServer.Data;
using EPiServer.Notification;
using EPiServer.Notification.Internal;
using EPiServer.Security;

namespace AdvancedTaskManager.Infrastructure.Cms
{
    public interface INotificationHandler
    {
        Task<ContentTask> GetNotifications(string id, ContentTask customTask, bool isContentQuery);
    }

    public class NotificationHandler : INotificationHandler
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IAsyncDatabaseExecutor _databaseExecutor;
        public NotificationHandler(IUserNotificationRepository userNotificationRepository, IAsyncDatabaseExecutor databaseExecutor)
        {
            _userNotificationRepository = userNotificationRepository;
            _databaseExecutor = databaseExecutor;
        }
        public async Task<ContentTask> GetNotifications(string id, ContentTask customTask, bool isContentQuery)
        {
            if (PrincipalAccessor.Current.Identity != null && !string.IsNullOrEmpty(PrincipalAccessor.Current.Identity.Name))
            {
                var notifications = await GetNotifications(PrincipalAccessor.Current.Identity.Name, id, isContentQuery);

                if (notifications?.PagedResult != null && notifications.PagedResult.Any())
                {
                    //Mark Notification Read
                    foreach (var notification in notifications.PagedResult)
                    {
                        await _userNotificationRepository.MarkUserNotificationAsReadAsync(new NotificationUser(PrincipalAccessor.Current.Identity.Name), notification.ID);
                    }

                    customTask.NotificationUnread = true;
                }
                else
                {
                    customTask.NotificationUnread = false;
                }
            }

            return customTask;
        }

        private async Task<PagedInternalNotificationMessageResult> GetNotifications(string user, string contentId, bool isContentQuery = true)
        {
            return new PagedInternalNotificationMessageResult(await _databaseExecutor.ExecuteAsync(async () =>
            {
                var entries = new List<InternalNotificationMessage>();
                var query = "SELECT pkID AS ID, Recipient, Sender, Channel, [Type], [Subject], Content, Sent, SendAt, Saved, [Read], Category FROM [tblNotificationMessage] " +
                            $"WHERE Recipient = '{user}'";

                if (isContentQuery)
                {
                    query = query + $"AND Content like '%\"contentLink\":\"{contentId}_%' " +
                            "AND Content like '%status\":7%' " +
                            "AND Channel = 'epi-approval' ";
                }
                else
                {
                    query = query + $"AND Content like '%\"ApprovalID\": {contentId},%' " +
                            "AND Channel = 'epi-changeapproval' ";
                }

                query = query + "AND [Read] is NULL " +
                        "order by Saved desc";

                var command = _databaseExecutor.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync())
                    {
                        entries.Add(Create(reader));
                    }
                }
                return (IList<InternalNotificationMessage>)entries;
            }).ConfigureAwait(false), 0L);
        }


        private static InternalNotificationMessage Create(DbDataReader reader)
        {
            var notificationMessage = new InternalNotificationMessage()
            {
                Recipient = Convert.ToString(reader["Recipient"]),
                Sender = Convert.ToString(reader["Sender"]),
                ChannelName = Convert.ToString(reader["Channel"]),
                TypeName = Convert.ToString(reader["Type"]),
                Subject = Convert.ToString(reader["Subject"]),
                Content = Convert.ToString(reader["Content"]),
                ID = Convert.ToInt32(reader["ID"]),
                Saved = Convert.ToDateTime(reader["Saved"]).ToLocalTime()
            };
            var obj1 = reader["Sent"];
            if (obj1 != DBNull.Value)
                notificationMessage.Sent = Convert.ToDateTime(obj1).ToLocalTime();

            var obj2 = reader["SendAt"];
            if (obj2 != DBNull.Value)
                notificationMessage.SendAt = Convert.ToDateTime(obj2).ToLocalTime();

            var obj3 = reader["Read"];
            if (obj3 != DBNull.Value)
                notificationMessage.Read = Convert.ToDateTime(obj3).ToLocalTime();

            var obj4 = reader["Category"];
            if (obj4 != DBNull.Value)
                notificationMessage.Category = new Uri(Convert.ToString(obj4) ?? string.Empty);

            return notificationMessage;
        }
    }
}
