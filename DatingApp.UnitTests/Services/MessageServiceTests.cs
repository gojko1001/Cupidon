using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services;
using DatingApp.SignalR;
using DatingApp.Utils.Pagination;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.UnitTests.Services
{
    [TestFixture]
    public class MessageServiceTests
    {
        private MessageService _messageService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IHubContext<PresenceHub>> _presenceHub;
        private Mock<PresenceTracker> _presenceTracker;

        private CreateMessageDto _createMessageDto = new CreateMessageDto { SenderUsername = "alice", RecipientUsername = "bob", Content = "MessageContent" };

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _presenceHub = new Mock<IHubContext<PresenceHub>>();
            _presenceTracker = new Mock<PresenceTracker>();

            _messageService = new MessageService(_unitOfWork.Object, _presenceHub.Object, _presenceTracker.Object);
        }

        [Test]
        public async Task GetMessagesForUser_WhenCalled_ReturnPagedListMessageDto()
        {
            var messageParams = new MessageParams();
            _unitOfWork.Setup(u => u.MessageRepository.GetMessagesForUserAsync(It.IsAny<MessageParams>())).Verifiable();

            await _messageService.GetMessagesForUser(messageParams);

            _unitOfWork.Verify(u => u.MessageRepository.GetMessagesForUserAsync(messageParams));
        }


        [Test]
        public async Task GetMessageThread_WhenCalled_ReturnMessageThreadBetweenUsersAsync()
        {
            _unitOfWork.Setup(u => u.MessageRepository.GetMessageThread("alice", "bob")).Returns(Task.FromResult<IEnumerable<Message>>(new List<Message>
            {
                new Message { Id = 1, SenderUsername = "bob", RecipientUsername = "alice" },
                new Message { Id = 2, SenderUsername = "alice", RecipientUsername = "bob" },
                new Message { Id = 3, SenderUsername = "bob", RecipientUsername = "alice" },
            }));

            var result = (List<Message>)await _messageService.GetMessageThread("alice", "bob");

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetMessageThread_WhenCalled_SetReadDateOfAllUnreadMessagesSentToCurrentUser()
        {
            var dateRead = new DateTime(2022, 1, 10, 14, 0, 0);
            _unitOfWork.Setup(u => u.MessageRepository.GetMessageThread("alice", "bob")).Returns(Task.FromResult<IEnumerable<Message>>(new List<Message>
            {
                new Message { Id = 1, RecipientUsername = "alice"},
                new Message { Id = 2, RecipientUsername = "bob"},
                new Message { Id = 3, RecipientUsername = "alice", DateRead = dateRead}
            }));

            var result = (List<Message>) await _messageService.GetMessageThread("alice", "bob");

            Assert.That(result.Find(x => x.Id == 1).DateRead, Is.Not.Null);
            Assert.That(result.Find(x => x.Id == 2).DateRead, Is.Null);
            Assert.That(result.Find(x => x.Id == 3).DateRead, Is.EqualTo(dateRead));
        }



        [Test]
        public void SendMessage_SendToNonExistingUser_ThrowNotFoundException()
        {
            SetUpExistingUsers();
            Assert.That(async () 
                => await _messageService.SendMessage(
                    new CreateMessageDto { SenderUsername = "alice", RecipientUsername = "non_existing_user_username" }), 
                Throws.Exception.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task SendMessage_SendToExistingUser_ReturnSentMessageAsync()
        {
            SetUpExistingUsers();
            _unitOfWork.Setup(u => u.GroupRepository.GetMessageGroup(It.IsAny<string>())).Returns(Task.FromResult(new Group()));
            _unitOfWork.Setup(u => u.MessageRepository.AddMessage(It.IsAny<Message>()));

            var result = await _messageService.SendMessage(_createMessageDto);

            Assert.That(result.SenderUsername, Is.EqualTo(_createMessageDto.SenderUsername));
            Assert.That(result.RecipientUsername, Is.EqualTo(_createMessageDto.RecipientUsername));
            Assert.That(result.Content, Is.EqualTo(_createMessageDto.Content));
        }

        [Test]
        public async Task SendMessage_SendToExistingUser_SaveMessageToDB()
        {
            SetUpExistingUsers();
            _unitOfWork.Setup(u => u.GroupRepository.GetMessageGroup(It.IsAny<string>())).Returns(Task.FromResult(new Group()));
            _unitOfWork.Setup(u => u.MessageRepository.AddMessage(It.IsAny<Message>())).Verifiable();

            var result = await _messageService.SendMessage(_createMessageDto);

            _unitOfWork.Verify(u => u.MessageRepository.AddMessage(It.IsAny<Message>()));
        }

        [Test]
        public async Task SendMessage_WhileRecepientIsInChat_SetDateReadOfMessageAsync()
        {
            SetUpExistingUsers();
            _unitOfWork.Setup(u => u.GroupRepository.GetMessageGroup(It.IsAny<string>())).Returns(Task.FromResult(new Group
            {
                Connections = new List<Connection> { 
                    new Connection { Username = "alice" },
                    new Connection { Username = "bob" } 
                }
            }));
            _unitOfWork.Setup(u => u.MessageRepository.AddMessage(It.IsAny<Message>()));

            var result = await _messageService.SendMessage(_createMessageDto);

            Assert.That(result.DateRead, Is.Not.Null);
        }

        [Test]
        [Ignore("Presence Tracker cannot be mocked this way")]

        public async Task SendMessage_WhileRecepientIsNotInChat_InvokeNotificationToRecepient()
        {
            SetUpExistingUsers();
            var connections = new List<string>();
            _unitOfWork.Setup(u => u.GroupRepository.GetMessageGroup(It.IsAny<string>())).Returns(Task.FromResult(new Group()));
            _presenceTracker.Setup(t => t.GetConnectionsForUser("bob")).Returns(Task.FromResult(connections));
            _unitOfWork.Setup(u => u.MessageRepository.AddMessage(It.IsAny<Message>()));

            var result = await _messageService.SendMessage(_createMessageDto);

            _presenceHub.Verify(h => h.Clients.Clients(connections));
        }



        [Test]
        public void RemoveMessage_MessageDoesntExist_ThrowInvalidActionException()
        {
            _unitOfWork.Setup(u => u.MessageRepository.GetMessageAsync(1)).Returns(Task.FromResult((Message)null));

            Assert.That(async () => await _messageService.RemoveMessage(1, "alice"), Throws.Exception.TypeOf<InvalidActionException>());
        }

        [Test]
        public void RemoveMessage_MessageSenderOrRecepiantNotMatchesUsername_ThrowUnauthorizedException()
        {
            _unitOfWork.Setup(u => u.MessageRepository.GetMessageAsync(1)).Returns(Task.FromResult(new Message
            {
                Id = 1,
                Sender = new AppUser { UserName = "bob" },
                Recipient = new AppUser { UserName = "john" }
            }));

            Assert.That(async () => await _messageService.RemoveMessage(1, "alice"), Throws.Exception.TypeOf<UnauthorizedException>());
        }

        [Test]
        public async Task RemoveMessage_MessageExists_RemoveMessageFromDBAsync()
        {
            var message = new Message
            {
                Id = 1,
                Sender = new AppUser { UserName = "alice" },
                Recipient = new AppUser { UserName = "bob" },
                SenderDeleted = true,
                RecipientDeleted = true,
            };
            _unitOfWork.Setup(u => u.MessageRepository.GetMessageAsync(1)).Returns(Task.FromResult(message));
            _unitOfWork.Setup(u => u.Complete()).Returns(Task.FromResult(true));

            await _messageService.RemoveMessage(1, "alice");

            _unitOfWork.Verify(u => u.MessageRepository.RemoveMessage(message));
        }

        [Test]
        public async Task RemoveMessage_SenderDeletes_SetSenderDeletedToTrue()
        {
            var message = new Message
            {
                Id = 1,
                Sender = new AppUser { UserName = "alice" },
                Recipient = new AppUser { UserName = "bob" },
                SenderDeleted = false,
                RecipientDeleted = false,
            };
            _unitOfWork.Setup(u => u.MessageRepository.GetMessageAsync(1)).Returns(Task.FromResult(message));
            _unitOfWork.Setup(u => u.Complete()).Returns(Task.FromResult(true));

            var result = await _messageService.RemoveMessage(1, "alice");

            Assert.That(result.SenderDeleted, Is.True);
        }

        [Test]
        public async Task RemoveMessage_RecepientDeletes_SetRecepientDeletedToTrue()
        {
            var message = new Message
            {
                Id = 1,
                Sender = new AppUser { UserName = "alice" },
                Recipient = new AppUser { UserName = "bob" },
                SenderDeleted = false,
                RecipientDeleted = false,
            };
            _unitOfWork.Setup(u => u.MessageRepository.GetMessageAsync(1)).Returns(Task.FromResult(message));
            _unitOfWork.Setup(u => u.Complete()).Returns(Task.FromResult(true));

            var result = await _messageService.RemoveMessage(1, "bob");

            Assert.That(result.RecipientDeleted, Is.True);
        }

        private void SetUpExistingUsers()
        {
            _unitOfWork.Setup(u => u.UserRepository.GetUserByUsernameAsync("alice")).Returns(Task.FromResult(new AppUser
            {
                Id = 1,
                UserName = "alice"
            }));
            _unitOfWork.Setup(u => u.UserRepository.GetUserByUsernameAsync("bob")).Returns(Task.FromResult(new AppUser
            {
                Id = 2,
                UserName = "bob"
            }));
        }
    }
}
