using Xinchen.XLinq;
using Xinchen.DbEntity;
namespace UnitTestProject1
{
	public partial class EducationGameEntities:QueryContext
	{
		public EducationGameEntities():base("test")
		{}
		public QueryEntitySet<UserConfig> UserConfigs { get;private set; }
		public QueryEntitySet<Department> Departments { get;private set; }
		public QueryEntitySet<User> Users { get;private set; }
		public QueryEntitySet<UserRole> UserRoles { get;private set; }
		public QueryEntitySet<RolePrivilege> RolePrivileges { get;private set; }
		public QueryEntitySet<Privilege> Privileges { get;private set; }
		public QueryEntitySet<RoleMenu> RoleMenus { get;private set; }
		public QueryEntitySet<Role> Roles { get;private set; }
		public QueryEntitySet<DependenceStages> DependenceStageses { get;private set; }
		public QueryEntitySet<Player> Players { get;private set; }
		public QueryEntitySet<Menu> Menus { get;private set; }
		public QueryEntitySet<Stages> Stageses { get;private set; }
		public QueryEntitySet<Task> Tasks { get;private set; }
		public QueryEntitySet<DependenceTask> DependenceTasks { get;private set; }
		public QueryEntitySet<Language> Languages { get;private set; }
		public QueryEntitySet<TaskExampleLanguage> TaskExampleLanguages { get;private set; }
		public QueryEntitySet<ProjectTemplate> ProjectTemplates { get;private set; }
		public QueryEntitySet<PlayerFolder> PlayerFolders { get;private set; }
		public QueryEntitySet<Sequence> Sequences { get;private set; }
		}
}
