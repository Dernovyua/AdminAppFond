using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models
{
    public class Client : ObservableObject
    {
        private int id;
        private string fullName = string.Empty;
        private string accountNumber = string.Empty;
        private DateTime openedAt;
        private string status = "active";
        private string? notes;
        private DateTime createdAt;
        private DateTime updatedAt;

        /// <summary>
        /// Уникальный идентификатор клиента.
        /// </summary>
        public int Id
        {
            get => id;
            set => SetField(ref id, value);
        }

        /// <summary>
        /// ФИО клиента.
        /// </summary>
        public string FullName
        {
            get => fullName;
            set => SetField(ref fullName, value);
        }

        /// <summary>
        /// Уникальный номер счета клиента.
        /// </summary>
        public string AccountNumber
        {
            get => accountNumber;
            set => SetField(ref accountNumber, value);
        }

        /// <summary>
        /// Дата открытия счета.
        /// </summary>
        public DateTime OpenedAt
        {
            get => openedAt;
            set => SetField(ref openedAt, value);
        }

        /// <summary>
        /// Статус счета: active, blocked, closed.
        /// </summary>
        public string Status
        {
            get => status;
            set => SetField(ref status, value);
        }

        /// <summary>
        /// Дополнительные заметки по клиенту.
        /// </summary>
        public string? Notes
        {
            get => notes;
            set => SetField(ref notes, value);
        }

        /// <summary>
        /// Дата создания записи.
        /// </summary>
        public DateTime CreatedAt
        {
            get => createdAt;
            set => SetField(ref createdAt, value);
        }

        /// <summary>
        /// Дата последнего обновления записи.
        /// </summary>
        public DateTime UpdatedAt
        {
            get => updatedAt;
            set => SetField(ref updatedAt, value);
        }
    }
}
