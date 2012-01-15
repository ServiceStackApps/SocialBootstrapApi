# Bash completion script for invoke-operation
#
# To use, run the following:
#   source `pwd`/nuget_bash_completion.sh

_invoke_operation_completion()
{
    local cur="${COMP_WORDS[COMP_CWORD]}"

    # Subcommand list
    [[ ${COMP_CWORD} -eq 1 ]] && {
        local actions="delete install list pack publish push setApiKey sources spec update help"
        COMPREPLY=( $(compgen -W "${actions}" -- ${cur}) )
        return
    }

    # Find the previous non-switch word
    local prev_index=$((COMP_CWORD - 1))
    local prev="${COMP_WORDS[prev_index]}"
    while [[ $prev == -* ]]; do
        prev_index=$((--prev_index))
        prev="${COMP_WORDS[prev_index]}"
    done

    case "$prev" in
    # base commands for nuget
    delete|install|list|pack|publish|push|setApiKeysources|spec|update|help)
        # handle standard -options
        if [[ "$prev" == "help" && "$cur" == -* ]]; then
            local opts=$(
                local opts=( TODO
        		 	)
                for o in ${opts[*]}; do
                    [[ " ${COMP_WORDS[*]} " =~ " $o " ]] || echo "$o"
                done
            )
            COMPREPLY=( $(compgen -W "$opts" -- ${cur}) )
            return
        fi
        local ff=$(\ls ./packages | sed -e "s/\.dll//g")
         COMPREPLY=( $(compgen -W "${ff}" -- ${cur}) )
        return
        ;;
    esac
}

complete -o bashdefault -o default -F _invoke_operation_completion nuget  
