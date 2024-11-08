﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Beneficiario
    /// </summary>
    public class BeneficiarioModel
    {

        public long Id { get; set; }
        /// <summary>
        /// CPF
        /// </summary>
        public string CPF { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        public string Nome { get; set; }
      
        public long IdCliente { get; set; }
    }
}