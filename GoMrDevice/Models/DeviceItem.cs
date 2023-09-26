﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMrDevice.Models
{
	public class DeviceItem
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string DeviceMessage { get; set; } = null!;
	}
}
