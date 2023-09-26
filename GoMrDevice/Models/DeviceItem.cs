using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMrDevice.Models
{
	public class DeviceItem
	{
		public string? DeviceId { get; set; }
		public string? DeviceType { get; set; }
		public string? Placement { get; set; }
		public bool IsActive { get; set; } = false;
		public string Icon => SetIcon();

		private string SetIcon()
		{
			return (DeviceType?.ToLower()) switch
			{
				"light" => "\uf0eb",
				"fan" => "\ue004",
				_ => "\uf2db",
			};
		}
	}
}
