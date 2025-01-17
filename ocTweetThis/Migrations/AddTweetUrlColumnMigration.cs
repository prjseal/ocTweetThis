﻿using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace ocTweetThis.Migrations
{
    public class AddTweetUrlColumnMigration : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IScopeProvider _scopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public AddTweetUrlColumnMigration(
            IScopeProvider scopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _migrationPlanExecutor = migrationPlanExecutor;
            _scopeProvider = scopeProvider;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            var migrationPlan = new MigrationPlan("UrlColumnToDb");

            // This is the steps we need to take
            // Each step in the migration adds a unique value
            migrationPlan.From(string.Empty)
                .To<AddTweetUrlColumn>("addTweetUrlColumn");

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(
                _migrationPlanExecutor,
                _scopeProvider,
                _keyValueService);
        }

    }

    public class AddTweetUrlColumn : MigrationBase
    {
        public AddTweetUrlColumn(IMigrationContext context) : base(context)
        {

        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "AddTweetUrlColumn");
            if(TableExists("TweetsPublished"))
            {
                if(ColumnExists("TweetsPublished", "TweetUrl") == false)
                {
                    Create.Column("TweetUrl").OnTable("TweetsPublished").AsString().Nullable().Do();
                }
                else
                {
                    Logger.LogDebug("Column TweetUrl already exists, new column not created");
                }
            }
        }
    }

   
}
