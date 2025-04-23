using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace nBanks.Application.ChatHistories
{
    public class UpdateChatHistoryDTO
    {
        public List<string> Questions { get; set; }

        public List<string> Answers { get; set; }

        public UpdateChatHistoryDTO(List<string> questions, List<string> answers)
        {
            Questions = questions ?? throw new ArgumentNullException(nameof(questions), "Questions cannot be null.");
            Answers = answers ?? throw new ArgumentNullException(nameof(answers), "Answers cannot be null.");
        }
    }
}
