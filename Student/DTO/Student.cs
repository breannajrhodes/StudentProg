using System;
using System.ComponentModel;

namespace StudentLib.DTO
{
	public class Student : INotifyPropertyChanged
	{

		private int m_Id;
		private string m_FirstName;
		private string m_LastName;
		private DateTime m_BirthDate;
		private double m_GPA;
		private int m_CreditsCompleted;
		private string m_Major;

		public int Id
		{
			get
			{
				return m_Id;
			}
			set
			{
				if (value != m_Id)
				{
					m_Id = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
				}
			}
		}
		public string FirstName
		{
			get
			{
				return m_FirstName;
			}
			set
			{
				if (value != m_FirstName)
				{
					m_FirstName = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
				}
			}
		}
		public string LastName
		{
			get
			{
				return m_LastName;
			}
			set
			{
				if (value != m_LastName)
				{
					m_LastName = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
				}
			}
		}
		public DateTime BirthDate
		{
			get
			{
				return m_BirthDate;
			}
			set
			{
				if (value != m_BirthDate)
				{
					m_BirthDate = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthDate)));
				}
			}
		}
		public double GPA
		{
			get
			{
				return m_GPA;
			}
			set
			{
				if (value != m_GPA)
				{
					m_GPA = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GPA)));
				}
			}
		}
		public int CreditsCompleted
		{
			get
			{
				return m_CreditsCompleted;
			}
			set
			{
				if (value != m_CreditsCompleted)
				{
					m_CreditsCompleted = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreditsCompleted)));
				}
			}
		}
		public string Major
		{
			get
			{
				return m_Major;
			}
			set
			{
				if (value != m_Major)
				{
					m_Major = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Major)));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Student(int id, string firstName, string lastName, DateTime birthDate, double gpa, int creditsCompleted, string major)
		{
			if (id >= 100000 && id <= 999999)
				Id = id;
			else
				throw new Exception("student ID number not 6 digits long");
			FirstName = firstName;
			LastName = lastName;
			BirthDate = birthDate;
			GPA = gpa;
			CreditsCompleted = creditsCompleted;
			Major = major;
		}

        public override string ToString()
        {
			string id = "ID: " + Id;
			string name = "Name: " + FirstName + " " + LastName;
			string birthday = "Birthdate: " + BirthDate.ToShortDateString();
			string gpa = "GPA: " + GPA;
			string creditsCompleted = "Credits Completed: " + CreditsCompleted;
			string major = "Major: " + Major;

			String returnString = String.Format("{0, -10} {1, -25} {2, -10}, {3, -10}, {4, -10}, {5, -15} \n", id, name, birthday, gpa, creditsCompleted, major);

			return returnString;
        }

    }

}